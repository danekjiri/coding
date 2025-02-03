<?php

namespace Core\Models;

use DateTimeImmutable;

class Article {
    private ?int $id;
    private string $name;
    private string $content;
    private string $createdAt;
    private string $updatedAt;

    public function __construct(?int $id, string $name, string $content = '', string $createdAt, string $updatedAt) {
        $this->id = $id;
        $this->name = $name;
        $this->content = $content;
        $this->createdAt = $createdAt;
        $this->updatedAt = $updatedAt;
    }

    public function getId(): ?int {
        return $this->id;
    }

    public function getName(): string {
        return $this->name;
    }

    public function setName(string $name): void {
        $this->name = $name;
    }

    public function getContent(): string {
        return $this->content;
    }

    public function setContent(string $content): void {
        $this->content = $content;
    }

    public function getCreatedAt(): string {
        return $this->createdAt;
    }

    public function getUpdatedAt(): string {
        return $this->updatedAt;
    }
}