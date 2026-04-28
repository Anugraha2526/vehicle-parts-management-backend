import axiosClient from "./axiosClient";

export const staffApi = {
  list: () => axiosClient.get("/api/admin/staff"),
};
