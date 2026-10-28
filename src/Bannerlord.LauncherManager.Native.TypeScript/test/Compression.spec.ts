import test from 'ava';
import { randomBytes } from 'crypto';
import { inflateRawSync } from 'zlib';

import { Compression } from '../src';

const CHUNK = 1024 * 1024;

// Text like the game's XML, it compresses well
const text = (size: number): Buffer => {
  const line = Buffer.from('<SubModule><Name value="Bannerlord.Harmony"/><DLLName value="Bannerlord.Harmony.dll"/></SubModule>\n');
  return Buffer.concat(Array.from({ length: Math.ceil(size / line.length) }, () => line)).subarray(0, size);
};

// Compressed the way the archives are: chunks at the same time, joined in their order
const deflateChunksAsync = async (data: Buffer): Promise<Buffer> => {
  const chunks: Buffer[] = [];
  for (let offset = 0; offset < data.length || chunks.length === 0; offset += CHUNK) {
    chunks.push(data.subarray(offset, offset + CHUNK));
  }
  const compressed = await Promise.all(chunks.map((chunk, index) => Compression.deflateRawChunkAsync(chunk, index === chunks.length - 1)));
  return Buffer.concat(compressed);
};

test('Chunks compressed at the same time join into one deflate stream', async (t) => {
  const data = Buffer.concat([text(3 * CHUNK), randomBytes(CHUNK / 2), text(CHUNK / 3)]);
  const compressed = await deflateChunksAsync(data);

  t.true(compressed.length < data.length * 0.4);
  t.deepEqual(inflateRawSync(compressed), data);
});

test('What doesn\'t compress is stored, with 5 bytes per 64 KB', async (t) => {
  const data = randomBytes(2 * CHUNK + 12345);
  const compressed = await deflateChunksAsync(data);

  t.true(compressed.length <= data.length + Math.ceil(data.length / 65535) * 5 + 3 * 16);
  t.deepEqual(inflateRawSync(compressed), data);
});

test('A chunk that isn\'t the last doesn\'t end the stream', async (t) => {
  const first = await Compression.deflateRawChunkAsync(text(1000), false);
  t.throws(() => inflateRawSync(first), { message: /unexpected end of file/ });

  const end = await Compression.deflateRawChunkAsync(new Uint8Array(0), true);
  t.deepEqual(inflateRawSync(Buffer.concat([first, end])), text(1000));
});

test('An empty stream is one empty final chunk', async (t) => {
  const compressed = await Compression.deflateRawChunkAsync(new Uint8Array(0), true);
  t.is(inflateRawSync(compressed).length, 0);
});

test('A view into a larger buffer compresses only its bytes, which it leaves as they were', async (t) => {
  const data = text(10000);
  const before = Buffer.from(data);
  const view = new Uint8Array(data.buffer, data.byteOffset + 100, 5000);

  const compressed = await Compression.deflateRawChunkAsync(view, true);
  t.deepEqual(inflateRawSync(compressed), before.subarray(100, 5100));
  t.deepEqual(data, before);
});

test('A chunk that isn\'t a Uint8Array is refused', (t) => {
  t.throws(() => Compression.deflateRawChunkAsync('text' as unknown as Uint8Array, true), { instanceOf: TypeError });
  t.throws(() => Compression.deflateRawChunkAsync(new Uint16Array(4) as unknown as Uint8Array, true), { instanceOf: TypeError });
});
