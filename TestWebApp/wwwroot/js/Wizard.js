function nextStep(currentStep) 
{
    const currentElement = document.getElementById('step' + currentStep);
    const nextStep = currentStep + 1;
    const nextElement = document.getElementById('step' + nextStep);
    
    if (nextElement) 
    {
        currentElement.style.display = 'none';
        nextElement.style.display = 'block';
    }
}

function prevStep(currentStep) 
{
    const currentElement = document.getElementById('step' + currentStep);
    const prevStep = currentStep - 1;
    const prevElement = document.getElementById('step' + prevStep);

    if (prevElement) 
    {
        currentElement.style.display = 'none';
        prevElement.style.display = 'block';
    }
}
const dropArea = document.getElementById("drop-area");
const fileInput = document.getElementById("fileInput");
const browseButton = document.getElementById("browseButton");
const fileNamesList = document.getElementById("fileNames");
const statusDiv = document.getElementById("status");

function displayFileNames(files) {
    fileNamesList.innerHTML = "";
    for (let i = 0; i < files.length; i++) {
        const li = document.createElement("li");
        li.textContent = files[i].name;
        fileNamesList.appendChild(li);
    }
}

browseButton.addEventListener("click", () => {
    fileInput.click();
});

fileInput.addEventListener("change", (event) => {
    displayFileNames(event.target.files);
});

dropArea.addEventListener("dragover", (event) => {
    event.preventDefault();
    dropArea.style.backgroundColor = "#f0f0f0";
});

dropArea.addEventListener("dragleave", () => {
    dropArea.style.backgroundColor = "#fff";
});

dropArea.addEventListener("drop", (event) => {
    event.preventDefault();
    dropArea.style.backgroundColor = "#fff";
    displayFileNames(event.dataTransfer.files);
});

dropArea.addEventListener("drop", (event) => {
    event.preventDefault();
    uploadFiles(event.dataTransfer.files);
});

browseButton.addEventListener("click", () => {
    uploadFiles(fileInput.files);
});
