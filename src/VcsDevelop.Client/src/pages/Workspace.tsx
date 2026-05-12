import React from 'react';
import Header from "../components/GlobalComponents/Header.tsx";
import LoginBody from "../components/LoginPage/LoginBody.tsx";
import Footer from "../components/GlobalComponents/Footer.tsx";
import WorkspaceHeader from "../components/GlobalComponents/WorkspaceHeader.tsx";
import Repositories from "../components/Workspace/Repositories.tsx";

const Workspace = () => {
    return (
        <div>
            <WorkspaceHeader/>
            <Repositories/>
            <Footer/>
        </div>
    );
};

export default Workspace;