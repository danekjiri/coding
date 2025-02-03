<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>Article List</title>
    <link rel="stylesheet" href="<?= \Core\Config::BASE_PATH ?>/public/css/articles-list.css">
    <link rel="stylesheet" href="<?= \Core\Config::BASE_PATH ?>/public/css/styles.css">
    <link rel="icon" type="image/x-icon" href="<?= \Core\Config::BASE_PATH . '/public/img/favicon.ico' ?>">
    <script src="<?= \Core\Config::BASE_PATH ?>/public/js/articles-list.js" defer></script>
    <script src="<?= \Core\Config::BASE_PATH ?>/public/js/time-update.js" defer></script>
</head>
<body>

    <div id="main">
        <h1>Article list</h1>
        <hr>
        <table id="articleTable">
            <colgroup> <col class="articleName"> <col class="articleActions"> <col class="articleUpdatedAt"> </colgroup>
            <thead>
                <tr><th>Name</th><th>Actions</th><th>Last modified</th></tr>
            </thead>
            <tbody id="articleTableBody">
            <?php foreach ($articles as $article): ?>
                <tr data-article-id="<?= htmlspecialchars($article->getId()) ?>">
                    <td class="articleName" ><?= htmlspecialchars($article->getName()) ?></td>
                    <td class="articleActions">
                        <span class="actions">
                            <a href="<?= \Core\Config::BASE_PATH ?>/article/<?= $article->getId() ?>">Show</a>
                            <a href="<?= \Core\Config::BASE_PATH ?>/article-edit/<?= $article->getId() ?>">Edit</a>
                            <a id="deleteArticle" href="#" >Delete</a>
                        </span>
                    </td>
                    <td class="timeUpdate" data-time="<?= htmlspecialchars($article->getUpdatedAt()) ?>"></td>
                </tr>
            <?php endforeach; ?>
            </tbody>
        </table>
        <hr>

        <div id="paginationControls">
            <button id="btnPrev">Previous</button>
            <span id="pageInfo"></span>
            <button id="btnNext">Next</button>
            
            <button id="btnCreateArticle">Create article</button>
        </div>
    </div>

    <!-- Hidden modal for creation -->
    <div id="createDialog" class="modal" style="display:none;">
        <div class="modal-content">
            <label class="newArticleName" for="newArticleName">Article name:</label>
            <input class="newArticleName" type="text" id="newArticleName" maxlength="32">
            <div class="buttons">
                <button id="btnDialogCreate">Create</button>
                <button id="btnDialogCancel">Cancel</button>
            </div>
        </div>
    </div>

    <!-- Hidden form for new article creation -->
    <form id="createForm" action="<?= \Core\Config::BASE_PATH ?>/article" method="post" style="display:none;">
        <input type="hidden" name="name" id="hiddenNameField" value="">
    </form>
</body>
</html>
