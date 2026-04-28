export default function ErrorMessage({ message = "Something went wrong." }) {
  return <p style={{ color: "crimson" }}>{message}</p>;
}
