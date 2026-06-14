import React, {useState, useEffect, useRef, useCallback} from 'react';
import { useLocation } from 'react-router';
import { useFileViewer } from '../../hooks/useFileViewer.ts';

interface RepoEntry {
    name: string;
    path: string;
    type: string;
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

    const {
        isFileModalOpen,
        fileData,
        isFileLoading,
        openFile,
        closeFileModal
    } = useFileViewer(repoId, token);

    const [filesList, setFilesList] = useState<RepoEntry[]>([]);
    const [currentPath, setCurrentPath] = useState<string>('');
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [isDragging, setIsDragging] = useState<boolean>(false);
    const [isUploading, setIsUploading] = useState<boolean>(false);
    const fileInputRef = useRef<HTMLInputElement>(null);

    const host = 'http://localhost:5050';
    const treeUrl = currentPath
        ? `${host}/api/repos/${repoId}/tree?path=${encodeURIComponent(currentPath)}`
        : `${host}/api/repos/${repoId}/tree`;
    const uploadUrl = `${host}/api/repos/${repoId}/upload`;
    const stageUrl = `${host}/api/repos/${repoId}/stage`;
    const commitUrl = `${host}/api/repos/${repoId}/commit`;

    const fetchRepositoryTree = useCallback(async () => {
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
    }, [repoId, treeUrl, token]);


    useEffect(() => {
        if (repoId) {
            fetchRepositoryTree();
        }
    }, [repoId, fetchRepositoryTree]);

    const handleUploadPipeline = async (files: FileList | null) => {
        if (!files || files.length === 0 || !repoId) return;

        setIsUploading(true);

        try {
            for (let i = 0; i < files.length; i++) {
                const file = files[i];

                const formData = new FormData();
                formData.append('File', file);

                const uploadResponse = await fetch(uploadUrl, {
                    method: 'POST',
                    headers: {
                        Authorization: `Bearer ${token || ''}`
                    },
                    body: formData
                });

                if (!uploadResponse.ok) {
                    throw new Error(`Не удалось загрузить файл ${file.name}. Статус: ${uploadResponse.status}`);
                }

                const uploadData = await uploadResponse.json();
                const uploadId = typeof uploadData === 'string' ? uploadData : uploadData.id || uploadData.uploadId;

                if (!uploadId) {
                    throw new Error(`Отсутствует uploadId для файла ${file.name}`);
                }

                const stageResponse = await fetch(stageUrl, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        Authorization: `Bearer ${token || ''}`
                    },
                    body: JSON.stringify({
                        uploadId: uploadId,
                        repositoryPath: currentPath
                    })
                });

                if (!stageResponse.ok) {
                    throw new Error(`Не удалось проиндексировать (stage) файл ${file.name}`);
                }
            }

            const commitResponse = await fetch(commitUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token || ''}`
                },
                body: JSON.stringify({
                    message: `uploaded ${files.length} file(s) via web ui`
                })
            });

            if (commitResponse.ok) {
                alert('Файлы успешно загружены, застейджены и закоммичены!');
                fetchRepositoryTree();
            } else {
                alert(`Ошибка при создании коммита. Статус: ${commitResponse.status}`);
            }

        } catch (error) {
            console.error('Ошибка в процессе публикации файлов');
            alert('Произошла непредвиденная ошибка при загрузке');
        } finally {
            setIsUploading(false);
        }
    };

    const handleEntryClick = (entry: RepoEntry) => {
        if (entry.type === 'directory') {
            setCurrentPath(entry.path);
            return;
        }

        openFile(entry.path);
    };

    const handleGoUp = () => {
        if (!currentPath) return;

        const parentPath = currentPath.split('/').slice(0, -1).join('/');
        setCurrentPath(parentPath);
    };

    const handleDrop = (e: React.DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        setIsDragging(false);
        handleUploadPipeline(e.dataTransfer.files);
    };

    const handleDragOver = (e: React.DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        setIsDragging(true);
    };

    if (!repoId) {
        return <div className="repo-loading">Ошибка: Неверный идентификатор репозитория.</div>;
    }

    if (isLoading) {
        return <div className="repo-loading">Загрузка дерева репозитория...</div>;
    }

    const isEmpty = filesList.length === 0;

    return (
        <div className="repo-container">
            <input
                type="file"
                ref={fileInputRef}
                className="repo-hidden-input"
                multiple
                onChange={(e) => handleUploadPipeline(e.target.files)}
            />

            {isUploading && (
                <div className="repo-loading" style={{ color: '#c618d3', fontWeight: 'bold' }}>
                    Выполняется загрузка, индексация и коммит файлов...
                </div>
            )}

            {isEmpty ? (
                <div className="repo-empty-wrapper">
                    <h3>{currentPath ? 'Папка пуста' : 'Репозиторий пуст'}</h3>
                    {currentPath && (
                        <button onClick={handleGoUp} className="primary-btn small">
                            назад
                        </button>
                    )}

                    <div
                        onDragOver={handleDragOver}
                        onDragLeave={() => setIsDragging(false)}
                        onDrop={handleDrop}
                        className={`repo-drop-zone ${isDragging ? 'active' : ''}`}
                    >
                        <p className="repo-drop-text">
                            {isDragging ? 'отпустите файлы сюда' : 'перетащите файлы сюда для загрузки'}
                        </p>
                    </div>

                    <button onClick={() => fileInputRef.current?.click()} className="primary-btn">
                        выбрать файлы вручную
                    </button>
                </div>
            ) : (
                <div>
                    <div className="repo-header">
                        <h2>{currentPath ? currentPath : 'Содержимое репозитория'}</h2>
                        {currentPath && (
                            <button onClick={handleGoUp} className="primary-btn small">
                                назад
                            </button>
                        )}
                        <button onClick={() => fileInputRef.current?.click()} className="primary-btn small">
                            загрузить файл
                        </button>
                    </div>

                    <div className="repo-content-box">
                        <ul className="repo-list">
                            {/* Шапка таблицы — убираем класс интерактивности клика */}
                            <li className="repo-list-header">
                                <span>Название</span>
                                <span className="repo-file-type">Тип</span>
                            </li>
                            {filesList.map((file) => (
                                <li
                                    key={file.path}
                                    className="repo-list-item clickable"
                                    onClick={() => handleEntryClick(file)}
                                >
                                    <span>{file.name}</span>
                                    <span className="repo-file-type">{file.type}</span>
                                </li>
                            ))}
                        </ul>
                    </div>
                </div>
            )}

            {/* Модальное окно просмотра содержимого файла */}
            {isFileModalOpen && (
                <div className="modal-overlay" onClick={closeFileModal}>
                    <div className="modal-content file-viewer-modal" onClick={(e) => e.stopPropagation()}>
                        <button className="modal-close" onClick={closeFileModal}>&times;</button>

                        {isFileLoading ? (
                            <div className="repo-loading">Загрузка содержимого файла...</div>
                        ) : fileData ? (
                            <div className="file-display-container">
                                <div className="file-viewer-header">
                                    <h3>{fileData.path.split('/').pop()}</h3>
                                    <span className="file-size-badge">Размер: {fileData.size} байт</span>
                                </div>
                                <hr />
                                {/* Тэг pre сохраняет форматирование кода и переносы строк */}
                                <pre className="file-content-view">
                                    {fileData.content}
                                </pre>
                            </div>
                        ) : (
                            <div className="repo-loading" style={{ color: 'red' }}>
                                Не удалось загрузить данные файла.
                            </div>
                        )}
                    </div>
                </div>
            )}
        </div>
    );
};

export default RepositoryBody;
