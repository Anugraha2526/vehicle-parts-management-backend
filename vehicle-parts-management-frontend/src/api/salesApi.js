import axiosClient from "./axiosClient";

export const salesApi = {
  list: () => axiosClient.get("/api/sales/invoices"),
};
