document.addEventListener('DOMContentLoaded', function () {
    const uploadSection = document.getElementById('upload-section');
    const fileInput = document.getElementById('file-upload');
    const compressButton = document.getElementById('compress-button');
    const decompressButton = document.getElementById('decompress-button');
    const downloadLink = document.getElementById('download-link');
    let uploadedFile = null;

    // Inicialmente, oculta los botones
    compressButton.style.display = 'none';
    decompressButton.style.display = 'none';

    uploadSection.addEventListener('dragover', function (event) {
        event.preventDefault();
        uploadSection.classList.add('drag-over');
    });

    uploadSection.addEventListener('dragleave', function (event) {
        event.preventDefault();
        uploadSection.classList.remove('drag-over');
    });

    uploadSection.addEventListener('drop', function (event) {
        event.preventDefault();
        uploadSection.classList.remove('drag-over');

        const files = event.dataTransfer.files;
        if (files.length > 0) {
            fileInput.files = files;
            handleFile(files[0]);
        }
    });

    fileInput.addEventListener('change', function (event) {
        const files = event.target.files;
        if (files.length > 0) {
            handleFile(files[0]);
        }
    });

    compressButton.addEventListener('click', function () {
        if (uploadedFile) {
            sendFileToEndpoint(uploadedFile, 'compress');
        }
    });

    decompressButton.addEventListener('click', function () {
        if (uploadedFile) {
            sendFileToEndpoint(uploadedFile, 'decompress');
        }
    });

    function handleFile(file) {
        if (file.name.endsWith('.txt') || file.name.endsWith('.huff')) {
            uploadedFile = file;
            downloadLink.style.display = 'none';
            showButtons(file.name);
        } else {
            alert("Please upload a .txt or .huff file.");
        }
    }

    function showButtons(fileName) {
        if (fileName.endsWith('.txt')) {
            compressButton.style.display = 'inline-block';
            decompressButton.style.display = 'none';
        } else if (fileName.endsWith('.huff')) {
            compressButton.style.display = 'none';
            decompressButton.style.display = 'inline-block';
        }
    }

    async function sendFileToEndpoint(file, action) {
        const formData = new FormData();
        formData.append('file', file);

        const response = await fetch(`https://1mk4hb55-7089.brs.devtunnels.ms/api/Huffman/${action}`, {
            method: 'POST',
            body: formData
        });

        if (response.ok) {
            const blob = await response.blob();
            const url = URL.createObjectURL(blob);
            downloadLink.href = url;
            downloadLink.download = action === 'compress' ? 'compressed.huff' : 'decompressed.txt';
            downloadLink.style.display = 'inline-block';
            downloadLink.textContent = `Download ${downloadLink.download}`;
        } else {
            alert(`Failed to ${action} file.`);
        }
    }
});
