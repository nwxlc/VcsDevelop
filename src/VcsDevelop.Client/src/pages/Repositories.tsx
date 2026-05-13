import React from 'react';
import WorkspaceHeader from "../components/Workspace/WorkspaceHeader"
import Footer from "../components/GlobalComponents/Footer.tsx";
import RepositoriesList from "../components/Workspace/RepositoriesList.tsx";

const Repositories = () => {
    return (
        <div className='container no-margin'>
            <WorkspaceHeader/>
            <RepositoriesList/>
            <Footer/>
        </div>
    );
};

export default Repositories;