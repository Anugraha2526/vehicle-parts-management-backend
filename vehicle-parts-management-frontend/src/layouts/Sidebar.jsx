import { Link } from "react-router-dom";

export default function Sidebar() {
  return (
    <aside>
      <nav>
        <Link to="/admin">Admin</Link>
      </nav>
    </aside>
  );
}
