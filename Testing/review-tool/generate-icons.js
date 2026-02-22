// Quick script to generate simple PNG icons for the extension.
// Run: node generate-icons.js
// Creates 16x16, 48x48, and 128x128 PNG icons in extension/icons/

const fs = require("fs");
const path = require("path");

// Minimal PNG encoder — creates a solid-color PNG with a letter "R" (review)
// Uses raw PNG format with IDAT containing uncompressed deflate blocks.

function createPng(size) {
  const canvas = Buffer.alloc(size * size * 4);

  // Fill with TrashMob green (#0e7c3a)
  for (let i = 0; i < size * size; i++) {
    canvas[i * 4] = 14; // R
    canvas[i * 4 + 1] = 124; // G
    canvas[i * 4 + 2] = 58; // B
    canvas[i * 4 + 3] = 255; // A
  }

  // Draw a simple "R" shape in white for larger sizes
  if (size >= 48) {
    const s = Math.floor(size / 16); // scale factor
    drawR(canvas, size, s);
  } else if (size >= 16) {
    drawR(canvas, size, 1);
  }

  // Make corners transparent for a rounded look
  const r = Math.floor(size * 0.15);
  for (let y = 0; y < size; y++) {
    for (let x = 0; x < size; x++) {
      // Check all 4 corners
      const corners = [
        [0, 0],
        [size - 1, 0],
        [0, size - 1],
        [size - 1, size - 1],
      ];
      for (const [cx, cy] of corners) {
        const dx = Math.abs(x - cx);
        const dy = Math.abs(y - cy);
        if (dx < r && dy < r) {
          const dist = Math.sqrt(
            (r - dx) * (r - dx) + (r - dy) * (r - dy)
          );
          if (dist > r) {
            canvas[(y * size + x) * 4 + 3] = 0; // transparent
          }
        }
      }
    }
  }

  return encodePng(canvas, size, size);
}

function drawR(canvas, size, scale) {
  const white = [255, 255, 255, 255];
  const s = scale;
  const ox = Math.floor(size * 0.3);
  const oy = Math.floor(size * 0.2);

  function setPixel(x, y) {
    for (let dy = 0; dy < s; dy++) {
      for (let dx = 0; dx < s; dx++) {
        const px = x * s + dx + ox;
        const py = y * s + dy + oy;
        if (px >= 0 && px < size && py >= 0 && py < size) {
          const idx = (py * size + px) * 4;
          canvas[idx] = white[0];
          canvas[idx + 1] = white[1];
          canvas[idx + 2] = white[2];
          canvas[idx + 3] = white[3];
        }
      }
    }
  }

  // Simple R glyph on a grid
  const h = Math.floor(size * 0.6 / s);
  const w = Math.floor(size * 0.4 / s);
  const thick = Math.max(1, Math.floor(w * 0.25));
  const mid = Math.floor(h * 0.45);

  // Left vertical bar
  for (let y = 0; y < h; y++) {
    for (let t = 0; t < thick; t++) setPixel(t, y);
  }
  // Top horizontal bar
  for (let x = 0; x < w; x++) {
    for (let t = 0; t < thick; t++) setPixel(x, t);
  }
  // Middle horizontal bar
  for (let x = 0; x < w; x++) {
    for (let t = 0; t < thick; t++) setPixel(x, mid + t);
  }
  // Right side of bump (top half)
  for (let y = thick; y < mid; y++) {
    for (let t = 0; t < thick; t++) setPixel(w - thick + t, y);
  }
  // Diagonal leg
  for (let i = 0; i < h - mid - thick; i++) {
    const x = thick + Math.floor((i * (w - thick)) / (h - mid - thick));
    for (let t = 0; t < thick; t++) {
      setPixel(x + t, mid + thick + i);
    }
  }
}

// Minimal PNG encoder
function encodePng(rgba, width, height) {
  function crc32(buf) {
    let c = 0xffffffff;
    const table = new Int32Array(256);
    for (let n = 0; n < 256; n++) {
      let val = n;
      for (let k = 0; k < 8; k++)
        val = val & 1 ? 0xedb88320 ^ (val >>> 1) : val >>> 1;
      table[n] = val;
    }
    for (let i = 0; i < buf.length; i++)
      c = table[(c ^ buf[i]) & 0xff] ^ (c >>> 8);
    return (c ^ 0xffffffff) >>> 0;
  }

  function adler32(buf) {
    let a = 1,
      b = 0;
    for (let i = 0; i < buf.length; i++) {
      a = (a + buf[i]) % 65521;
      b = (b + a) % 65521;
    }
    return ((b << 16) | a) >>> 0;
  }

  function chunk(type, data) {
    const len = Buffer.alloc(4);
    len.writeUInt32BE(data.length);
    const typeAndData = Buffer.concat([Buffer.from(type), data]);
    const crc = Buffer.alloc(4);
    crc.writeUInt32BE(crc32(typeAndData));
    return Buffer.concat([len, typeAndData, crc]);
  }

  // IHDR
  const ihdr = Buffer.alloc(13);
  ihdr.writeUInt32BE(width, 0);
  ihdr.writeUInt32BE(height, 4);
  ihdr[8] = 8; // bit depth
  ihdr[9] = 6; // color type (RGBA)
  ihdr[10] = 0; // compression
  ihdr[11] = 0; // filter
  ihdr[12] = 0; // interlace

  // Raw image data (filter byte 0 + row data for each row)
  const rowSize = width * 4 + 1;
  const rawData = Buffer.alloc(height * rowSize);
  for (let y = 0; y < height; y++) {
    rawData[y * rowSize] = 0; // filter: none
    rgba.copy(rawData, y * rowSize + 1, y * width * 4, (y + 1) * width * 4);
  }

  // Compress with store-only deflate (no compression, simple but works)
  const MAX_BLOCK = 65535;
  const blocks = [];
  let offset = 0;
  while (offset < rawData.length) {
    const remaining = rawData.length - offset;
    const blockSize = Math.min(remaining, MAX_BLOCK);
    const isLast = offset + blockSize >= rawData.length;
    const header = Buffer.alloc(5);
    header[0] = isLast ? 1 : 0;
    header.writeUInt16LE(blockSize, 1);
    header.writeUInt16LE(~blockSize & 0xffff, 3);
    blocks.push(header);
    blocks.push(rawData.subarray(offset, offset + blockSize));
    offset += blockSize;
  }

  // Zlib wrapper: CMF + FLG + blocks + ADLER32
  const cmf = 0x78;
  const flg = 0x01; // no dict, lowest compression
  const zlibHeader = Buffer.from([cmf, flg]);
  const adler = Buffer.alloc(4);
  adler.writeUInt32BE(adler32(rawData));
  const compressed = Buffer.concat([zlibHeader, ...blocks, adler]);

  // PNG file
  const signature = Buffer.from([137, 80, 78, 71, 13, 10, 26, 10]);
  return Buffer.concat([
    signature,
    chunk("IHDR", ihdr),
    chunk("IDAT", compressed),
    chunk("IEND", Buffer.alloc(0)),
  ]);
}

// Generate and write icons
const iconsDir = path.join(__dirname, "extension", "icons");
for (const size of [16, 48, 128]) {
  const png = createPng(size);
  fs.writeFileSync(path.join(iconsDir, `icon${size}.png`), png);
  console.log(`Created icon${size}.png (${png.length} bytes)`);
}

console.log("Done!");
