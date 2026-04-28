import axiosClient from "./axiosClient";

export const reportApi = {
  summary: () => axiosClient.get("/api/finance/reports/summary"),
};
