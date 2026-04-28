export default function Modal({ isOpen, children }) {
  if (!isOpen) {
    return null;
  }

  return <div role="dialog">{children}</div>;
}
