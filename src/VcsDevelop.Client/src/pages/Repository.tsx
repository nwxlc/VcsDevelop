import React from 'react';
import WorkspaceHeader from "../components/Workspace/WorkspaceHeader";
import RepositoryBody from "../components/Workspace/RepositoryBody";
import Footer from "../components/GlobalComponents/Footer";

const Repository = () => {
    return (
        <div className='container'>
            <WorkspaceHeader/>
            <RepositoryBody/>
            <Footer/>
        </div>
    );
};

export default Repository;