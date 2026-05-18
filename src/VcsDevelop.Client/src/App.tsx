import './App.css'
import { useEffect } from "react";
import { Routes, Route, useNavigate, useLocation } from "react-router";
import HomePage from "./pages/HomePage.tsx";
import Login from "./pages/Login.tsx";
import Workspace from "./pages/Workspace.tsx";
import { useAuth } from "./hooks/useAuth.ts";
import Repositories from "./pages/Repositories.tsx";
import PullRequests from "./pages/PullRequests.tsx";
import Repository from "./pages/Repository.tsx";

function App() {
    const navigate = useNavigate();
    const location = useLocation();
    const { refreshToken } = useAuth();

    useEffect(() => {
        const protectedRoutes = ["/workspace", "/repositories", "/repository", "/pullrequests"];
        const isProtectedRoute = protectedRoutes.some(route => location.pathname.startsWith(route));

        const verify = async () => {
            const token = localStorage.getItem("accessToken");
            const expiresAt = localStorage.getItem("accessTokenExpiresAt");

            const isExpired = expiresAt ? new Date(expiresAt).getTime() < Date.now() : true;

            if (isProtectedRoute && (!token || isExpired)) {
                console.log("Токен просрочен, запрашиваем новый...");

                const newToken = await refreshToken();

                if (newToken) {
                    console.log("Токен обновлен, перезагружаем страницу для обновления данных...");
                    window.location.reload();
                } else {
                    alert("Нужна повторная авторизация");
                    navigate("/login");
                }
            }
        };

        verify();
    }, [location.pathname, navigate, refreshToken]);

    return (
        <Routes>
            <Route path="/" element={<HomePage/>}/>
            <Route path="/login" element={<Login/>}/>
            <Route path="/workspace" element={<Workspace/>}/>
            <Route path="/repositories" element={<Repositories/>}/>
            <Route path="/repository/:name" element={<Repository/>} />
            <Route path="/pullrequests" element={<PullRequests/>}/>
        </Routes>
    )
}

export default App;