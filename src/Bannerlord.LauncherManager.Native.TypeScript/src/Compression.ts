import * as types from './types';

export class Compression {
  private static addon: types.ICompression;

  /* istanbul ignore next */
  private constructor() { }

  private static initialize() {
    if (Compression.addon === undefined) {
      Compression.addon = require('./../launchermanager.node');
    }
  }

  /**
   * Compresses one chunk of a raw deflate stream (RFC 1951) with ISA-L level 1, on libuv's thread pool.
   *
   * A chunk is compressed without the chunks before it. One that isn't the last ends byte aligned without a final
   * block, so the outputs of a stream's chunks joined in their order are one deflate stream, the last chunk ends it.
   * Chunks of one stream can be compressed at the same time. What doesn't compress is stored, the output is at most
   * the chunk with 5 bytes per 64 KB.
   *
   * The chunk's memory is read while the promise is pending, it must not be changed until the promise settles.
   */
  public static deflateRawChunkAsync = (chunk: Uint8Array, last: boolean): Promise<Buffer> => {
    Compression.initialize();
    return Compression.addon.deflateRawChunkAsync(chunk, last);
  }
}
