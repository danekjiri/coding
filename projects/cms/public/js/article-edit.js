function validateForm(e) {
    const nameInput = document.getElementById('articleName');
    const contentInput = document.getElementById('articleContent');
    if (nameInput.value.trim() === '') {
        e.preventDefault();
        alert("Article name cannot be empty!");
    } else if (nameInput.value.length > 32) {
        e.preventDefault();
        alert("Article name cannot be more than 32 characters!\nCurrent length: " + nameInput.value.length);
    } else if (contentInput.value.length > 1024) {
        e.preventDefault();
        alert("Article content cannot be more than 1024 characters!\nCurrent length: " + contentInput.value.length);
    }
}