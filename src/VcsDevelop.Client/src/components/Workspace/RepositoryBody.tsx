import React, { useState, useEffect, useRef } from 'react';
import { useLocation } from 'react-router';

// Описываем структуру файла, основанную на поле "entries" из скриншота
interface RepoEntry {
    name: string;
    path: string;
    type: string; // "file" или "dir" / "string"
    blobId: string | null;
}

interface LocationState {
    id: string;
}

const RepositoryBody: React.FC = () => {
    const location = useLocation();
    const state = location.state as LocationState | null;
    const repoId = state?.id;

    const token = localStorage.getItem('accessToken');

    const [filesList, setFilesList] = useState<RepoEntry[]>([]);
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [isDragging, setIsDragging] = useState<boolean>(false);
    const fileInputRef = useRef<HTMLInputElement>(null);

    const treeUrl = `/api/repos/${repoId}/tree`;
    const uploadUrl = `/api/repos/${repoId}/upload`;

    const fetchRepositoryTree = async () => {
        if (!repoId) return;
        setIsLoading(true);
        try {
            const response = await fetch(treeUrl, {
                headers: {
                    Authorization: `Bearer ${token || ''}`
                }
            });
            if (response.ok) {
                const data = await response.json();

                // Согласно схеме, файлы лежат в свойстве data.entries
                if (data && Array.isArray(data.entries)) {
                    setFilesList(data.entries);
                } else {
                    setFilesList([]);
                }
            } else {
                console.error("Ошибка при получении дерева репозитория:", response.status);
                setFilesList([]);
            }
        } catch (error) {
            console.error('Ошибка загрузки дерева данных:', error);
            setFilesList([]);
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        if (repoId) {
            fetchRepositoryTree();
        }
    }, [repoId]);

    const uploadFiles = async (files: FileList | null) => {
        if (!files || files.length === 0 || !repoId) return;

        const formData = new FormData();
        for (let i = 0; i < files.length; i++) {
            formData.append('File', files[i]);
        }

        try {
            const response = await fetch(uploadUrl, {
                method: 'POST',
                headers: {
                    Authorization: `Bearer ${token || ''}`
                },
                body: formData
            });

            if (response.ok) {
                alert('Успешно загружено!');
                // После успешной загрузки обновляем дерево файлов
                fetchRepositoryTree();
            } else {
                alert(`Ошибка при загрузке файлов. Статус: ${response.status}`);
            }
        } catch (error) {
            console.error('Ошибка отправки:', error);
        }
    };

    const handleDrop = (e: React.DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        setIsDragging(false);
        uploadFiles(e.dataTransfer.files);
    };

    const handleDragOver = (e: React.DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        setIsDragging(true);
    };

    if (!repoId) {
        return <div className="repo-loading">Ошибка: Неверный идентификатор репозитория.</div>;
    }

    if (isLoading) {
        return <div className="repo-loading">Загрузка...</div>;
    }

    const isEmpty = filesList.length === 0;

    return (
        <div className="repo-container">
            <input
                type="file"
                ref={fileInputRef}
                className="repo-hidden-input"
                multiple
                onChange={(e) => uploadFiles(e.target.files)}
            />

            {isEmpty ? (
                <div className="repo-empty-wrapper">
                    <h3>Репозиторий пуст</h3>

                    <div
                        onDragOver={handleDragOver}
                        onDragLeave={() => setIsDragging(false)}
                        onDrop={handleDrop}
                        className={`repo-drop-zone ${isDragging ? 'active' : ''}`}
                    >
                        <p className="repo-drop-text">
                            {isDragging ? 'Отпустите файлы сюда' : 'Перетащите файлы сюда для загрузки'}
                        </p>
                    </div>

                    <button onClick={() => fileInputRef.current?.click()} className="repo-btn-big">
                        Выбрать файлы вручную
                    </button>
                </div>
            ) : (
                <div>
                    <div className="repo-header">
                        <h2>Содержимое репозитория</h2>
                        <button onClick={() => fileInputRef.current?.click()} className="repo-btn-small">
                            Загрузить файл
                        </button>
                    </div>

                    <div className="repo-content-box">
                        <ul className="repo-list">
                            {filesList.map((file, index) => (
                                <li key={file.blobId || index} className="repo-list-item">
                                    {/* Выводим имя файла/папки из объекта entries */}
                                    {file.name}
                                    <span style={{ fontSize: '12px', color: '#888', marginLeft: '10px' }}>
                                        ({file.type})
                                    </span>
                                </li>
                            ))}
                        </ul>
                    </div>
                </div>
            )}
        </div>
    );
};

export default RepositoryBody;