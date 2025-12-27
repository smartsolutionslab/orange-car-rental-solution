/**
 * Image URL - branded type for type safety
 * Matches backend ImageUrl value object
 * Valid absolute HTTP/HTTPS URLs with image extensions
 */
export type ImageUrl = string & { readonly __brand: 'ImageUrl' };

const IMAGE_EXTENSIONS = /\.(jpg|jpeg|png|gif|webp|svg)(\?.*)?$/i;
const MAX_LENGTH = 500;

export function isImageUrl(value: string): value is ImageUrl {
  if (value.length > MAX_LENGTH) return false;
  try {
    const url = new URL(value);
    return (url.protocol === 'http:' || url.protocol === 'https:') &&
           IMAGE_EXTENSIONS.test(url.pathname);
  } catch {
    return false;
  }
}

export function createImageUrl(value: string): ImageUrl {
  const trimmed = value.trim();
  if (!isImageUrl(trimmed)) {
    throw new Error(`Invalid ImageUrl format: ${value}`);
  }
  return trimmed;
}

/**
 * Safely convert a string to ImageUrl, returning undefined if invalid
 */
export function toImageUrl(value: string | null | undefined): ImageUrl | undefined {
  if (!value) return undefined;
  const trimmed = value.trim();
  return isImageUrl(trimmed) ? trimmed : undefined;
}
