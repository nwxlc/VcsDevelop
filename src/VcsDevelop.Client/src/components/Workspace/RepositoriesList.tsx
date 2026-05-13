import React, { useState } from 'react';
import { useNavigate } from 'react-router';

const RepositoriesList = () => {
    const navigate = useNavigate();
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [repoName, setRepoName] = useState('');
    const [createdRepoId, setCreatedRepoId] = useState(null);
    const [isLoading, setIsLoading] = useState(false);
    
    const accessToken = localStorage.getItem('accessToken');
    
    const repositories = [
        { id: 'd73b2762-1234-4a51-9c32-1f7498302abc', name: 'frontend-core-library' },
        { id: 'a42f8c12-5678-4b21-8d14-2e8509413def', name: 'data-processing-service' },
        { id: 'b91e7a34-9012-4c31-7e25-3f9610524ghi', name: 'mobile-ui-kit' },
        { id: 'c12d6b56-3456-4d41-6f36-4a0721635jkl', name: 'auth-provider-v2' },
        { id: 'e53f5d78-7890-4e51-5a47-5b1832746mno', name: 'analytics-dashboard' },
        { id: 'f64a4c90-1234-4f61-4b58-6c2943857pqr', name: 'deployment-scripts' },
        { id: 'g75b3b01-5678-4g71-3c69-7d3054968stu', name: 'legacy-api-gateway' },
        { id: 'h86c2a12-9012-4h81-2d70-8e4165079vwx', name: 'testing-framework-ext' },
        { id: 'i97d1e23-3456-4i91-1e81-9f5276180yz1', name: 'documentation-assets' },
        { id: 'j08e0f34-7890-4j01-0f92-0a6387291ab2', name: 'experimental-ai-module' },
    ];

    const handleRepoClick = (repo) => {
        navigate(`/repository/${repo.name}`, {
            state: { id: repo.id }
        });
    };
    
    const handleCreate = async () => {
        if (!repoName.trim()) return;
        setIsLoading(true);
        try {
            const response = await fetch('/api/repos/create', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${accessToken}`
                },
                body: JSON.stringify({
                    name: repoName,
                    defaultBranchName: null,
                    description: null,
                    tags: ['']
                })
            });

            if (response.ok) {
                const data = await response.json();
                setCreatedRepoId(data.id || 'some-id');
            }
        } catch (error) {
            console.error('Ошибка при создании:', error);
        } finally {
            setIsLoading(false);
        }
    };

    const closeModal = () => {
        setIsModalOpen(false);
        setRepoName('');
        setCreatedRepoId(null);
    };

    return (
        <div className="page-wrapper">
            <div className="header-section">
                <h2 className="title">ваши репозитории</h2>
                <button className="add-btn" onClick={() => setIsModalOpen(true)}>
                    добавить репозиторий
                </button>
            </div>

            <div className="repositories-container">
                {repositories.map((repo) => (
                    <div
                        key={repo.id}
                        className="repository-card"
                        onClick={() => handleRepoClick(repo)} 
                    >
                        <span className="repository-name">{repo.name}</span>
                    </div>
                ))}
            </div>

            {/* Модальное окно */}
            {isModalOpen && (
                <div className="modal-overlay" onClick={closeModal}>
                    <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                        <button className="modal-close" onClick={closeModal}>&times;</button>

                        {!createdRepoId ? (
                            <>
                                <h3>Новый репозиторий</h3>
                                <input
                                    type="text"
                                    className="modal-input"
                                    placeholder="введите название..."
                                    value={repoName}
                                    onChange={(e) => setRepoName(e.target.value)}
                                />
                                <button
                                    className="submit-btn"
                                    onClick={handleCreate}
                                    disabled={isLoading}
                                >
                                    {isLoading ? 'Создание...' : 'Создать'}
                                </button>
                            </>
                        ) : (
                            <div className="success-message">
                                <h3>Успешно создано!</h3>
                                <div className="modal-actions">
                                    <button className="secondary-btn" onClick={closeModal}>
                                        Закрыть
                                    </button>
                                    <button
                                        className="primary-btn"
                                        onClick={() => {
                                            navigate(`/repository/${repoName}`, {
                                                state: { id: createdRepoId }
                                            });
                                            closeModal();
                                        }}
                                    >
                                        Перейти
                                    </button>
                                </div>
                            </div>
                        )}
                    </div>
                </div>
            )}
        </div>
    );
};

export default RepositoriesList;