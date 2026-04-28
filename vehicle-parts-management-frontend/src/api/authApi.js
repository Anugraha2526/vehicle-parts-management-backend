import axiosClient from "./axiosClient";

export const authApi = {
  login: (payload) => axiosClient.post("/api/identity/login", payload),
};
