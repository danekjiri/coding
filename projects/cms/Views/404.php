<!DOCTYPE html>
    <html>
        <head>
            <meta charset="utf-8">
            <link rel="icon" type="image/x-icon" href="<?= \Core\Config::BASE_PATH . '/public/img/favicon.ico' ?>">
            <title>404</title>
        </head>
        <body style="display: flex; justify-content: center;">
            <div class="center-container" style="text-align: center">
                <h1>404 Not Found</h1>
                <?php if(isset($messageToShow)): ?>
                    <p><?= $messageToShow ?></p>
                <?php else: ?>
                    <p>Sorry, the page you are looking for could not be found.</p>
                <?php endif; ?>
                <img src='<?= \Core\Config::BASE_PATH . '/public/img/duke404.jpg' ?>' alt="404 Image">
            </div>
        </body>
    </html>