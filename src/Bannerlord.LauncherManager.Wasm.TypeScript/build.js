// Builds the WebAssembly runtime and the TS wrapper.
//   build          the runtime and the wrapper
//   build-dotnet   the runtime only, into dist/runtime: launchermanager.wasm and launchermanager.html, the JS hosting it
//   build-ts       the wrapper only, into dist/main
//   test           builds the tests and runs them against dist/runtime
// The runtime needs the .NET 10 SDK and its wasm-tools workload (dotnet workload install wasm-tools)
const { execSync } = require("child_process");
const fs = require("fs");
const path = require("path");

const ROOT = __dirname;
const DOTNET_PROJECT_DIR = path.resolve(ROOT, "../Bannerlord.LauncherManager.Wasm");
const PUBLISH_DIR = path.join(ROOT, "build-dotnet");
const RUNTIME_DIR = path.join(ROOT, "dist", "runtime");

// NativeAOT-LLVM's dotnet.runtime.js decodes every string .NET returns one char at a time, `s += String.fromCharCode(c)`:
// a JS object per char, 75 MB of garbage for a 750K char module list. The runtime's utf16ToString uses a TextDecoder when
// strings_init made one, but NativeAOT skips strings_init for its Mono parts and with it the decoder; this puts it back.
// Unlike the official runtime's, the decoder is fatal and the loop decodes what it rejects: a lone surrogate (valid in an
// NTFS name) stays as it is instead of becoming U+FFFD. A view of shared memory is copied, TextDecoder refuses those.
// Matched by its shape, the names are minified; a build without it fails rather than ships
const UTF16_LOOP = /let ([\w$]+)="";const ([\w$]+)=([\w$]+)\(\);for\(let ([\w$]+)=([\w$]+);\4<([\w$]+);\4\+=2\)\{const ([\w$]+)=[\w$]+\(\2,\4\);\1\+=String\.fromCharCode\(\7\)\}return \1/;
const utf16DecoderPlugin = {
  name: "utf16-decoder",
  transform(code, id) {
    if (!id.endsWith("dotnet.runtime.js")) return null;
    if (!UTF16_LOOP.test(code)) throw new Error("dotnet.runtime.js: its UTF-16 decoding loop wasn't found, update UTF16_LOOP in build.js");
    const patched = code.replace(UTF16_LOOP, (loop, _result, view, getView, _ptr, start, end) =>
      `try{const ${view}=${getView}();` +
      `return lmUtf16Decoder.decode(typeof SharedArrayBuffer!=="undefined"&&${view}.buffer instanceof SharedArrayBuffer` +
      `?${view}.slice(${start}>>>1,${end}>>>1):${view}.subarray(${start}>>>1,${end}>>>1))}catch{${loop}}`);
    return `const lmUtf16Decoder = new TextDecoder("utf-16le", { fatal: true });\n${patched}`;
  },
};

const run = (command, cwd = ROOT) => {
  console.log(`> ${command}`);
  execSync(command, { cwd, stdio: "inherit" });
};

const buildDotnet = async () => {
  fs.rmSync(PUBLISH_DIR, { recursive: true, force: true });
  // From the project's folder, its global.json picks the .NET 10 SDK. A build server of another SDK can't build it
  run(`dotnet publish -c Release -o "${PUBLISH_DIR}" --disable-build-servers`, DOTNET_PROJECT_DIR);

  fs.rmSync(RUNTIME_DIR, { recursive: true, force: true });
  fs.mkdirSync(RUNTIME_DIR, { recursive: true });
  fs.copyFileSync(path.join(PUBLISH_DIR, "dotnet.native.wasm"), path.join(RUNTIME_DIR, "launchermanager.wasm"));

  // dotnet.js, the runtime modules and the host, one ES module inlined into a page. A page loads it into a frame, a
  // document of its own: one that inherits the page's CSP can't compile WebAssembly. See src/runtime.ts
  const { build } = require("rolldown");
  const { output } = await build({
    input: path.join(ROOT, "runtime-host", "host.js"),
    resolve: { alias: { "dotnet-js": PUBLISH_DIR } },
    plugins: [utf16DecoderPlugin],
    // Emscripten's glue imports these only in Node, at runtime
    external: ["module", "process"],
    output: { format: "esm", codeSplitting: false },
    write: false,
    logLevel: "warn",
  });
  // "</script" would end the element. In JS it's only valid in strings and regular expressions, where "<\/" is the same
  const script = output[0].code.replace(/<\/script/gi, "<\\/script");
  fs.writeFileSync(
    path.join(RUNTIME_DIR, "launchermanager.html"),
    `<!doctype html>\n<meta charset="utf-8">\n<script type="module">\n${script}</script>\n`,
  );
};

const buildTs = () => {
  fs.rmSync(path.join(ROOT, "dist", "main"), { recursive: true, force: true });
  run("npx tsc -p tsconfig.json");
};

const test = () => {
  fs.rmSync(path.join(ROOT, "dist", "test"), { recursive: true, force: true });
  fs.rmSync(path.join(ROOT, "dist", "src"), { recursive: true, force: true });
  run("npx tsc -p tsconfig.test.json");
  run("npx ava");
};

const commands = {
  clean: () => {
    fs.rmSync(path.join(ROOT, "dist"), { recursive: true, force: true });
    fs.rmSync(PUBLISH_DIR, { recursive: true, force: true });
  },
  build: async () => {
    await buildDotnet();
    buildTs();
  },
  "build-dotnet": buildDotnet,
  "build-ts": buildTs,
  test,
};

const command = commands[process.argv[2]];
if (!command) {
  console.error(`Usage: node build.js <${Object.keys(commands).join("|")}>`);
  process.exit(1);
}
Promise.resolve(command()).catch((e) => {
  console.error(e);
  process.exit(1);
});
