import { useNavigate } from "react-router";

let isRefreshing = false;
let refreshPromise: Promise<string | null> | null = null;

export const useAuth = () => {
    const navigate = useNavigate();

    const logout = () => {
        localStorage.removeItem("accessToken");
        localStorage.removeItem("accessTokenExpiresAt");
        localStorage.removeItem("refreshToken");
        navigate("/login"); 
    };

    const refreshToken = async (): Promise<string | null> => {
        if (isRefreshing && refreshPromise) {
            return refreshPromise;
        }

        const currentRefreshToken = localStorage.getItem("refreshToken");
        const accessToken = localStorage.getItem("accessToken");

        if (!currentRefreshToken) {
            logout();
            return null;
        }

        isRefreshing = true;

        refreshPromise = (async () => {
            try {
                const response = await fetch('http://localhost:5050/api/account/refresh_access_token', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${accessToken}`
                    },
                    body: JSON.stringify({ refreshToken: currentRefreshToken })
                });

                if (response.ok) {
                    const data = await response.json();

                    localStorage.setItem("accessToken", data.accessToken.value);
                    localStorage.setItem("accessTokenExpiresAt", data.accessToken.expirationDate);

                    if (data.refreshToken) {
                        localStorage.setItem("refreshToken", data.refreshToken.value);
                    }

                    return data.accessToken.value;
                } else {
                    console.warn(`Бэкенд вернул ошибку ${response.status} при рефреше`);
                    logout();
                    return null;
                }
            } catch (error) {
                console.error("Сетевая ошибка при попытке рефреша токена:", error);
                logout();
                return null;
            } finally {
                isRefreshing = false;
                refreshPromise = null;
            }
        })();

        return refreshPromise;
    };

    return { refreshToken, logout };
};