import './App.css'
import { useEffect } from "react";
import { Routes, Route, useNavigate, useLocation } from "react-router";
import HomePage from "./pages/HomePage.tsx";
import Login from "./pages/Login.tsx";
import Workspace from "./pages/Workspace.tsx";

function App() {
    const navigate = useNavigate();
    const location = useLocation();

    console.log(localStorage.getItem('accessToken'));

    useEffect(() => {
        const protectedRoutes = ["/workspace"];
        const isProtectedRoute = protectedRoutes.includes(location.pathname);
        const token = localStorage.getItem("accessToken");

        if (isProtectedRoute && !token) {
            alert("Авторизуйтесь, чтоб попасть на эту страницу");
            navigate("/", { replace: true });
        }
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