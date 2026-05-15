import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router';

const RepositoriesList = () => {
    const navigate = useNavigate();

    interface Repository {
        id: string;
        name: string;
    }
    
    // Состояния для данных
    const [repositories, setRepositories] = useState<Repository[]>([]);
    const [page, setPage] = useState(1);
    const [pageSize] = useState(10); // Можно вынести в константы
    const [totalCount, setTotalCount] = useState(0); // Если API возвращает общее кол-во

    // Состояния UI
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [repoName, setRepoName] = useState('');
    const [createdRepoId, setCreatedRepoId] = useState(null);
    const [isLoading, setIsLoading] = useState(false);
    

    

    const accessToken = localStorage.getItem('accessToken');

    // Функция загрузки данных
    const fetchRepositories = useCallback(async () => {
        setIsLoading(true);
        try {
            // Согласно скриншоту: GET /api/repos?page=X&pageSize=Y
            const response = await fetch(`http://localhost:5050/api/repos?page=${page}&pageSize=${pageSize}`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                const data = await response.json();

                // ВАЖНО: Проверьте структуру ответа вашего API. 
                // Если API возвращает массив напрямую: setRepositories(data);
                // Если объект с метаданными: setRepositories(data.items); setTotalCount(data.total);
                setRepositories(Array.isArray(data) ? data : data.items || []);
            }
        } catch (error) {
            console.error('Ошибка при загрузке репозиториев:', error);
        } finally {
            setIsLoading(false);
        }
    }, [page, pageSize, accessToken]);

    // Вызов загрузки при изменении страницы или монтировании
    useEffect(() => {
        fetchRepositories();
    }, [fetchRepositories]);

    const handleRepoClick = (repo) => {
        navigate(`/repository/${repo.name}`, {
            state: { id: repo.id }
        });
    };

    const handleCreate = async () => {
        if (!repoName.trim()) return;
        setIsLoading(true);
        try {
            const response = await fetch('http://localhost:5050/api/repos/create', {
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
                // Обновляем список, чтобы увидеть новый репозиторий
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
                    <p>Загрузка...</p>
                ) : (
                    repositories.map((repo) => (
                        <div
                            key={repo.id}
                            className="repository-card"
                            onClick={() => handleRepoClick(repo)}
                        >
                            <span className="repository-name">{repo.name}</span>
                        </div>
                    ))
                )}

                {!isLoading && repositories.length === 0 && (
                    <p>Репозитории не найдены</p>
                )}
            </div>

            {/* Блок пагинации */}
            <div className="pagination-controls">
                <button
                    disabled={page <= 1 || isLoading}
                    onClick={() => setPage(prev => prev - 1)}
                >
                    Назад
                </button>
                <span> Страница {page} </span>
                <button
                    disabled={repositories.length < pageSize || isLoading}
                    onClick={() => setPage(prev => prev + 1)}
                >
                    Вперед
                </button>
            </div>

            {/* Модальное окно (остается без изменений в логике, добавлена только очистка) */}
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