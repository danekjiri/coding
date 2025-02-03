<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>Article Detail</title>
    <link rel="icon" type="image/x-icon" href="<?= \Core\Config::BASE_PATH . '/public/img/favicon.ico' ?>">
    <link rel="stylesheet" href="<?= \Core\Config::BASE_PATH ?>/public/css/styles.css">
    <link rel="stylesheet" href="<?= \Core\Config::BASE_PATH ?>/public/css/article.css">
    <script src="<?= \Core\Config::BASE_PATH ?>/public/js/time-update.js" defer></script>
</head>
<body>
    <div id="main">
        <h1><?= htmlspecialchars($articleToShow->getName()) ?></h1>
        <div>
            <p><?= nl2br(htmlspecialchars($articleToShow->getContent())) ?></p>
        </div>
        <hr>
        <div>
            <span>
                <label> Created: </label>
                <text class='timeUpdate' data-time="<?= htmlspecialchars($articleToShow->getCreatedAt()) ?>"></text>
            </span>
            <br>
            <span>
                <label> Modified: </label>
                <text class='timeUpdate' data-time="<?= htmlspecialchars($articleToShow->getUpdatedAt()) ?>"></text>
            </span>
            <hr> <br>
        <div>
        <div id='navigations'>
            <a href="<?= \Core\Config::BASE_PATH ?>/article-edit/<?= $articleToShow->getId() ?>">
                <button id='editButton'>Edit</button>
            </a>
            <a href="<?= \Core\Config::BASE_PATH ?>/articles">
                <button id='backButton'>Back to articles</button>
            </a>
        </div>
    </div>
</body>
</html>
