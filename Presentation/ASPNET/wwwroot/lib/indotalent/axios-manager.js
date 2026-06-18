const AxiosManager = (() => {
    const axiosInstance = axios.create({
        baseURL: '/api',
        headers: {
            'accept': 'application/json',
            'Content-Type': 'application/json',
        }
    });

    let isRefreshing = false;
    let retryQueue = [];

    axiosInstance.interceptors.request.use(
        (config) => {
            const token = StorageManager.getAccessToken(); 
            if (token) {
                config.headers['Authorization'] = `Bearer ${token}`;
            }
            return config;
        },
        (error) => {
            return Promise.reject(error);
        }
    );

    axiosInstance.interceptors.response.use(
        (response) => response,
        async (error) => {
            const originalRequest = error.config;
            if (error.response && error.response.status === 498) {
                if (!isRefreshing) {
                    isRefreshing = true;

                    try {
                        const refreshToken = StorageManager.getRefreshToken();
                        const response = await axiosInstance.post('/Security/RefreshToken', { refreshToken });

                        if (response?.data?.code === 200) {
                            StorageManager.saveLoginResult(response?.data);
                            isRefreshing = false;
                            retryQueue.forEach((cb) => cb());
                            retryQueue = [];
                            return axiosInstance(originalRequest);
                        } else {
                            throw new Error('Refresh token failed');
                        }
                    } catch (refreshError) {
                        retryQueue.forEach((cb) => cb());
                        retryQueue = [];
                        isRefreshing = false;
                        throw refreshError;
                    }
                }

                return new Promise((resolve, reject) => {
                    retryQueue.push(() => {
                        resolve(axiosInstance(originalRequest));
                    });
                });
            }

            return Promise.reject(error);
        }
    );

    const request = async (method, url, data = {}, customHeaders = {}, responseType = 'json') => {
        try {
            const response = await axiosInstance({
                method,
                url,
                data,
                headers: {
                    ...customHeaders,
                },
                responseType,
            });
            return response;
        } catch (error) {
            throw error;
        }
    };

    const downloadBlob = (blob, filename) => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
    };

    const getFile = async (url, filename) => {
        const response = await axiosInstance.get(url, { responseType: 'blob' });
        downloadBlob(response.data, filename);
    };

    const postFile = async (url, formData, filename) => {
        const response = await axiosInstance.post(url, formData, {
            headers: { 'Content-Type': 'multipart/form-data' },
            responseType: 'blob',
        });
        if (response.data.type === 'application/json') {
            const text = await response.data.text();
            return JSON.parse(text);
        }
        downloadBlob(response.data, filename);
        return null;
    };

    return {
        request,
        get: (url, config = {}) => request('get', url, {}, config.headers, config.responseType),
        post: (url, data, config = {}) => request('post', url, data, config.headers, config.responseType),
        put: (url, data, config = {}) => request('put', url, data, config.headers, config.responseType),
        delete: (url, config = {}) => request('delete', url, {}, config.headers, config.responseType),
        getFile,
        postFile,
    };
})();
