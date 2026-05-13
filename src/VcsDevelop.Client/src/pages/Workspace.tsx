import React from 'react';
import Footer from "../components/GlobalComponents/Footer.tsx";
import WorkspaceHeader from "../components/Workspace/WorkspaceHeader.tsx";
import Account from "../components/Workspace/Account.tsx";

const Workspace = () => {
    return (
        <div className='container'>
            <WorkspaceHeader/>
            <Account/>
            <Footer/>
        </div>
    );
};

export default Workspace;