import axiosClient from "./axiosClient";

export const purchaseApi = {
  create: (payload) => axiosClient.post("/api/finance/purchases", payload),
  list: () => axiosClient.get("/api/finance/purchases"),
};
