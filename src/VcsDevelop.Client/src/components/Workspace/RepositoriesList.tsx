import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router';

interface Repository {
    id: string;
    name: string;
}

const RepositoriesList = () => {
    const navigate = useNavigate();

    // Состояния для данных
    const [repositories, setRepositories] = useState<Repository[]>([]);
    const [page, setPage] = useState(1);
    const [pageSize] = useState(10);
    const [totalPages, setTotalPages] = useState(1); // Храним общее число страниц из метаданных

    // Состояния UI
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [repoName, setRepoName] = useState('');
    const [createdRepoId, setCreatedRepoId] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState(false);

    const accessToken = localStorage.getItem('accessToken');

    const getPaginationRange = () => {
        const range: (number | string)[] = [];
        const delta = 2; 

        if (totalPages <= 7) {
            for (let i = 1; i <= totalPages; i++) range.push(i);
        } else {
            range.push(1);

            if (page > delta + 2) {
                range.push('...');
            }

            const start = Math.max(2, page - delta);
            const end = Math.min(totalPages - 1, page + delta);

            for (let i = start; i <= end; i++) {
                range.push(i);
            }

            if (page < totalPages - (delta + 1)) {
                range.push('...');
            }

            range.push(totalPages);
        }

        return range;
    };

    // Функция загрузки данных
    const fetchRepositories = useCallback(async () => {
        setIsLoading(true);
        try {
            const response = await fetch(`http://localhost:5050/api/repos?page=${page}&pageSize=${pageSize}`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                const result = await response.json();

                setRepositories(result.data || []);

                if (result.metadata) {
                    setTotalPages(result.metadata.totalPages || 1);
                }
            }
        } catch (error) {
            console.error('Ошибка при загрузке репозиториев:', error);
        } finally {
            setIsLoading(false);
        }
    }, [page, pageSize, accessToken]);

    useEffect(() => {
        fetchRepositories();
    }, [fetchRepositories]);

    const handleRepoClick = (repo: Repository) => {
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
                    defaultBranchName: "main",
                    description: "",
                    tags: []
                })
            });

            if (response.ok) {
                const data = await response.json();
                setCreatedRepoId(data.id || 'some-id');
                fetchRepositories();
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
                {isLoading && repositories.length === 0 ? (
                    <p className="status-text">Загрузка...</p>
                ) : repositories.length > 0 ? (
                    repositories.map((repo) => (
                        <div
                            key={repo.id}
                            className="repository-card"
                            onClick={() => handleRepoClick(repo)}
                        >
                            <span className="repository-name">{repo.name}</span>
                        </div>
                    ))
                ) : (
                    <p className="status-text">Репозитории не найдены</p>
                )}
            </div>

            {/* Блок пагинации */}
            <div className="pagination-controls">
                <button
                    className="page-arrow-btn"
                    disabled={page <= 1 || isLoading}
                    onClick={() => setPage(prev => prev - 1)}
                >
                    <svg viewBox="0 0 24 24" width="18" fill="white">
                        <path d="M15.41 7.41L14 6l-6 6 6 6 1.41-1.41L10.83 12z" />
                    </svg>
                </button>

                <div className="page-numbers">
                    {getPaginationRange().map((p, index) => (
                        <button
                            key={index}
                            className={`page-num-btn ${p === page ? 'active' : ''} ${p === '...' ? 'dots' : ''}`}
                            disabled={p === '...' || isLoading}
                            onClick={() => typeof p === 'number' && setPage(p)}
                        >
                            {p}
                        </button>
                    ))}
                </div>

                <button
                    className="page-arrow-btn"
                    disabled={page >= totalPages || isLoading}
                    onClick={() => setPage(prev => prev + 1)}
                >
                    <svg viewBox="0 0 24 24" width="18" fill="white">
                        <path d="M10 6L8.59 7.41 13.17 12l-4.58 4.59L10 18l6-6z" />
                    </svg>
                </button>
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
                                    <button className="secondary-btn" onClick={closeModal}>Закрыть</button>
                                    <button
                                        className="primary-btn"
                                        onClick={() => {
                                            navigate(`/repository/${repoName}`, { state: { id: createdRepoId } });
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