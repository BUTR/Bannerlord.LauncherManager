export interface ICompression {
    deflateRawChunkAsync(chunk: Uint8Array, last: boolean): Promise<Buffer>;
}
