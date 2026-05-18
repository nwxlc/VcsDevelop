import { useState } from 'react';

interface FileContentData {
    documentId: string;
    branchName: string;
    path: string;
    blobId: string;
    size: number;
    content: string;
}

export const useFileViewer = (repoId: string | undefined, token: string | null) => {
    const [isFileModalOpen, setIsFileModalOpen] = useState<boolean>(false);
    const [fileData, setFileData] = useState<FileContentData | null>(null);
    const [isFileLoading, setIsFileLoading] = useState<boolean>(false);

    const openFile = async (filePath: string) => {
        if (!repoId) return;

        setIsFileModalOpen(true);
        setIsFileLoading(true);
        setFileData(null); 

        try {
            const response = await fetch(`http://localhost:5050/api/repos/${repoId}/blob?path=${encodeURIComponent(filePath)}`, {
                method: 'GET',
                headers: {
                    Authorization: `Bearer ${token || ''}`
                }
            });

            if (response.ok) {
                const data: FileContentData = await response.json();
                setFileData(data);
            } else {
                console.error('Ошибка при загрузке содержимого файла:', response.status);
            }
        } catch (error) {
            console.error('Ошибка fetch запроса blob:', error);
        } finally {
            setIsFileLoading(false);
        }
    };

    const closeFileModal = () => {
        setIsFileModalOpen(false);
        setFileData(null);
    };

    return {
        isFileModalOpen,
        fileData,
        isFileLoading,
        openFile,
        closeFileModal
    };
};