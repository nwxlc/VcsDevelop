import { useNavigate } from "react-router";

export const useAuth = () => {
    const navigate = useNavigate();

    const refreshToken = async () => {
        const currentRefreshToken = localStorage.getItem("refreshToken");
        const accessToken = localStorage.getItem("accessToken");

        if (!currentRefreshToken) {
            logout();
            return null;
        }

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
                localStorage.setItem("accessToken", data.accessToken);
                return data.accessToken;
            } else {
                logout();
                return null;
            }
        } catch (error) {
            logout();
            return null;
        }
    };

    const logout = () => {
        localStorage.removeItem("accessToken");
        localStorage.removeItem("refreshToken");
        navigate("/");
    };

    return { refreshToken, logout };
};