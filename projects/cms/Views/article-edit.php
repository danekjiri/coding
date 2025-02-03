<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>Edit Article</title>
    <link rel="icon" type="image/x-icon" href="<?= \Core\Config::BASE_PATH . '/public/img/favicon.ico' ?>">
    <link rel="stylesheet" href="<?= \Core\Config::BASE_PATH ?>/public/css/styles.css">
    <link rel="stylesheet" href="<?= \Core\Config::BASE_PATH ?>/public/css/article-edit.css">
    <script src="<?= \Core\Config::BASE_PATH ?>/public/js/article-edit.js" defer></script>
</head>
<body>
    <div id="main">
        <h1>Edit Article</h1>
        <form   action="<?= \Core\Config::BASE_PATH ?>/article-edit/<?= $articleToEdit->getId() ?>" 
                method="post" 
                onsubmit="validateForm(event)">
            <div class="name">
                <label for="articleName">Name:</label>
                <br>
                <input  type="text" id="articleName" name="articleName" 
                        value="<?= htmlspecialchars($articleToEdit->getName()) ?>" maxlength="32" required />
            </div>
            <div class="content">
                <label for="articleContent">Content:</label>
                <br>
                <textarea id="articleContent" name="articleContent" cols="45" rows="15" maxlength="1024"><?= htmlspecialchars($articleToEdit->getContent()) ?></textarea>
            </div>
            <div>
                <button id="saveButton" type="submit">Save</button>
                <a href="<?= \Core\Config::BASE_PATH ?>/articles">
                    <button id="backButton" type="button">Back to articles</button>
                </a>
            </div>
        </form>
    </div>
</body>
</html>