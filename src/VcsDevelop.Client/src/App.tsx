import './App.css'
import { useEffect } from "react";
import { Routes, Route, useNavigate, useLocation } from "react-router";
import HomePage from "./pages/HomePage.tsx";
import Login from "./pages/Login.tsx";
import Workspace from "./pages/Workspace.tsx";
import {useAuth} from "./hooks/useAuth.ts";


function App() {
    const navigate = useNavigate();
    const location = useLocation();
    const { refreshToken } = useAuth();

    useEffect(() => {
        const protectedRoutes = ["/workspace"];
        const isProtectedRoute = protectedRoutes.includes(location.pathname);

        const verify = async () => {
            const token = localStorage.getItem("accessToken");
            if (isProtectedRoute && !token) {
                const success = await refreshToken();
                if (!success) {
                    alert("Нужна авторизация");
                    navigate("/login");
                }
            }
        };

        verify();
    }, [location.pathname, navigate]);

    return (
        <Routes>
            <Route path="/" element={<HomePage/>}/>
            <Route path="/login" element={<Login/>}/>
            <Route path="/workspace" element={<Workspace/>}/>
        </Routes>
    )
}

export default App