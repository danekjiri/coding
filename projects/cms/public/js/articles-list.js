(function() {
  // config and global variables
  const FIRST_PAGE = 1;
  const PAGE_SIZE = 10;

  let currentPage = 1;
  let allRows = [];
  
  // DOM elements
  const tbody = document.getElementById("articleTableBody");
  const btnPrev = document.getElementById("btnPrev");
  const btnNext = document.getElementById("btnNext");
  const pageInfo = document.getElementById("pageInfo");
  const createDialog = document.getElementById("createDialog");
  
  // init
  initRows();
  initPagination();
  initDeleteHandling();
  initCreateDialog();
  renderPage();

  // ---
  // HELPER FUNCTIONS 

  function initRows() {
    // Store existing table rows in allRows
    allRows = Array.prototype.slice.call(tbody.querySelectorAll("tr"));
  }

  // pagination
  function initPagination() {
    btnPrev.addEventListener("click", function() {
      if (currentPage > FIRST_PAGE) {
        currentPage--;
        renderPage();
      }
    });
    btnNext.addEventListener("click", function() {
      if (currentPage < totalPages()) {
        currentPage++;
        renderPage();
      }
    });
  }

  function totalPages() {
    // ensure at least 1 page if there are no rows
    return Math.max(1, Math.ceil(allRows.length / PAGE_SIZE));
  }

  function renderPage() {
    // ensure currentPage is in valid range
    if (currentPage > totalPages()) {
      currentPage = totalPages();
    } else if (currentPage < FIRST_PAGE) {
      currentPage = FIRST_PAGE;
    }

    // clear the table and append the correct slice
    tbody.innerHTML = "";
    const startIndex = (currentPage - 1) * PAGE_SIZE;
    const endIndex = startIndex + PAGE_SIZE;
    const currentPageRows = allRows.slice(startIndex, endIndex);

    for (var i = 0; i < currentPageRows.length; i++) {
      tbody.appendChild(currentPageRows[i]);
    }

    // update page info
    pageInfo.textContent = "Page " + currentPage + " of " + totalPages();

    // show/hide pagination buttons
    btnPrev.style.display = (currentPage <= FIRST_PAGE) ? "none" : "inline-block";
    btnNext.style.display = (currentPage >= totalPages()) ? "none" : "inline-block";
  }

  function initDeleteHandling() {
    tbody.addEventListener("click", function(event) {
      if (event.target && event.target.id === "deleteArticle") {
        event.preventDefault();
        const row = event.target.closest("tr");
        if (row) {
          const articleId = row.getAttribute("data-article-id");
          const articleName = row.querySelector(".articleName").textContent;
          deleteArticle(articleId, articleName);
        }
      }
    });
  }

  function deleteArticle(articleId, articleName) {
    if (!confirm(`Are you sure you want to delete article: '${articleName}'?`)) {
      return;
    }
    
    // ajax delete
    fetch("article/" + articleId, { method: "DELETE" })
      .then(function(response) {
        if (!response.ok) {
          return response.json().then(err => {
            throw new Error(err.error || "An unknown error occurred");
          });
        }
        return response.json();
      })
      .then(function(data) {
        if (data.status === "ok") {
          // find index by articleId
          const index = allRows.findIndex(function(tr) {
            return tr.getAttribute("data-article-id") == articleId;
          });
          // if valid index -> remove
          if (index >= 0 && index < allRows.length) {
            allRows.splice(index, 1);
          }
          console.log("Success:", data.message);
          renderPage();
        } else {
          alert("Delete error: " + JSON.stringify(data));
        }
      })
      .catch(function(err) {
        alert("Error deleting article: " + err.message);
      });
  }

  function initCreateDialog() {
    const btnCreateArticle = document.getElementById("btnCreateArticle");
    const btnDialogCreate = document.getElementById("btnDialogCreate");
    const btnDialogCancel = document.getElementById("btnDialogCancel");
    const newArticleName = document.getElementById("newArticleName");
    const hiddenNameField = document.getElementById("hiddenNameField");
    const createForm = document.getElementById("createForm");

    btnCreateArticle.addEventListener("click", function() {
      createDialog.style.display = "block";
      newArticleName.value = "";
      btnDialogCreate.disabled = true;
      newArticleName.focus();
    });

    btnDialogCancel.addEventListener("click", function() {
      createDialog.style.display = "none";
    });

    newArticleName.addEventListener("input", function() {
      btnDialogCreate.disabled = newArticleName.value.trim() === "";
    });

    btnDialogCreate.addEventListener("click", function() {
      const articleNameValue = newArticleName.value.trim();
      if (!articleNameValue) {
        alert("Article name cannot be empty!");
        return;
      } else if (articleNameValue.length > 32) {
        alert("Article name cannot be more than 32 characters!\nCurrent length: " + articleNameValue.length);
        return;
      }
      
      hiddenNameField.value = articleNameValue;
      createForm.submit();
    });
  }  
})();