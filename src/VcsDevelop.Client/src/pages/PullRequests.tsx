import React from 'react';
import WorkspaceHeader from "../components/Workspace/WorkspaceHeader.tsx";
import Footer from "../components/GlobalComponents/Footer.tsx";

const PullRequests = () => {
    return (
        <div className='container '>
            <WorkspaceHeader/>
            <h1 className="title">ваши запросы на слияние</h1>
            <Footer/>
        </div>
    );
};

export default PullRequests;