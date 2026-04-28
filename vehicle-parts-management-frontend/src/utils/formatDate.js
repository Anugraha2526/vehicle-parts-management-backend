export function formatDate(value, locale = "en-US") {
  if (!value) {
    return "";
  }

  return new Date(value).toLocaleDateString(locale);
}
