import React from 'react';

import Header from "../components/GlobalComponents/Header.tsx";
import Footer from "../components/GlobalComponents/Footer.tsx";
import HomePageBody from "../components/HomePage/HomePageBody.tsx";
const HomePage = () => {
    return (
        <div className="container">
            <Header/>
            <HomePageBody/>
            <Footer/>
        </div>
    );
};

export default HomePage;