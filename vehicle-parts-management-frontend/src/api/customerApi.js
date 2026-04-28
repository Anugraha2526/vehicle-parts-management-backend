import axiosClient from "./axiosClient";

export const customerApi = {
  list: () => axiosClient.get("/api/customer-crm/customers"),
};
