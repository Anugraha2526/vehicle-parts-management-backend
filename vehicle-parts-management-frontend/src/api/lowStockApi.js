import axiosClient from "./axiosClient";

export const lowStockApi = {
  list: () => axiosClient.get("/api/finance/low-stock"),
};
