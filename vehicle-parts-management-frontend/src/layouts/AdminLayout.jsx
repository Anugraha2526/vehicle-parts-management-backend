import { Outlet } from "react-router-dom";
import Navbar from "./Navbar";
import Sidebar from "./Sidebar";

export default function AdminLayout() {
  return (
    <div>
      <Navbar />
      <Sidebar />
      <main>
        <Outlet />
      </main>
    </div>
  );
}
