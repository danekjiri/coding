<?php

namespace Core\Models;

use PDO;
use PDOException;
use Core\Config;

class ArticleRepository {
    private PDO $pdo;

    public function __construct() {
        require Config::PROJECT_BASE . '/db_config.php';

        $dsn = 'mysql:host=' . $db_config['server']
             . ';dbname=' . $db_config['database']
             . ';charset=utf8';

        try {
            $this->pdo = new PDO($dsn, $db_config['login'], $db_config['password']);
            $this->pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
        } catch (PDOException $ex) {
            Config::debug('Database connection error: ' . $ex->getMessage());
            die();
        }
    }

    /**
     * Fetch all articles from DB.
     */
    public function getAll(): array {
        $sql = "SELECT * 
                FROM articles
                ORDER BY id";
        $statement= $this->pdo->prepare($sql);
        $statement->execute();
        $rows = $statement->fetchAll(PDO::FETCH_ASSOC);

        $articles = [];
        foreach ($rows as $row) {
            $article = new Article(
                (int)$row['id'],
                $row['name'],
                $row['content'] ?? '',
                $row['created_at'],
                $row['updated_at']
            );
            array_push($articles, $article);
        }
        return $articles;
    }

    /**
     * Fetch a single article by ID.
     */
    public function getById(int $id): ?Article {
        $sql = "SELECT * 
                FROM articles 
                WHERE id = :id";
        $statement = $this->pdo->prepare($sql);
        $statement->execute(['id' => $id]);

        $row = $statement->fetch(PDO::FETCH_ASSOC);
        if (!$row) {
            return null;
        }
        
        $article = new Article(
            (int)$row['id'],
            $row['name'],
            $row['content'] ?? '',
            $row['created_at'],
            $row['updated_at']
        );
        return $article;
    }

    /**
     * Insert a new article; returns the new ID.
     * The new article has the given name and empty content.
     */
    public function create(string $name): int {
        $sql = "INSERT 
                INTO articles (name, content) 
                VALUES (:name, '')";
        $statement = $this->pdo->prepare($sql);
        $statement->execute(['name' => $name]);

        $id = $this->pdo->lastInsertId();
        return $id;
    }

    /**
     * Update an existing article (name, content).
     */
    public function update(Article $article): void {
        $sql = "UPDATE articles
                SET name = :name,
                    content = :content
                WHERE id = :id";
        $statement = $this->pdo->prepare($sql);
        $statement->execute(
            [
                'name' => $article->getName(),
                'content' => $article->getContent(),
                'id' => $article->getId(),
            ]
        );
    }

    /**
     * Delete an article by ID.
     */
    public function delete(int $id): void {
        $sql = "DELETE 
                FROM articles
                WHERE id = :id";
        $statement = $this->pdo->prepare($sql);
        $statement->execute(['id' => $id]);
    }
}