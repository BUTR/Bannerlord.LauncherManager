// The native module handed out buffers in its own memory to avoid a copy. The runtime copies byte arrays either way,
// so these are plain arrays, kept for API compatibility

export const allocWithOwnership = (length: number): Uint8Array | null => new Uint8Array(length);

export const allocWithoutOwnership = (length: number): Uint8Array | null => new Uint8Array(length);

export const allocAliveCount = (): number => 0;
