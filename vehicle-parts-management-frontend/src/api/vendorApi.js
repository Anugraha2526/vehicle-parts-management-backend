import axiosClient from "./axiosClient";

export const vendorApi = {
  list: () => axiosClient.get("/api/finance/vendors"),
};
