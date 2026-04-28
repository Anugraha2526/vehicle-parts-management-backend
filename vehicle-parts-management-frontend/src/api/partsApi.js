import axiosClient from "./axiosClient";

export const partsApi = {
  list: () => axiosClient.get("/api/admin/parts"),
};
