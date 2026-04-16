import React from 'react';

import Header from "../components/GlobalComponents/Header.tsx";
import Footer from "../components/GlobalComponents/Footer.tsx";
import LoginBody from "../components/LoginPage/LoginBody.tsx";
const Login = () => {
    return (
        <div className="container">
            <Header/>
            <LoginBody/>
            <Footer/>
        </div>
    );
};

export default Login;