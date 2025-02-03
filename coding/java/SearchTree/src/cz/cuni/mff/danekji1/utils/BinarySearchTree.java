package cz.cuni.mff.danekji1.utils;

import java.util.Iterator;
import java.util.Stack;
import java.util.NoSuchElementException;

public class BinarySearchTree implements Iterable {
    private Node root;

    public BinarySearchTree() {
        root = null;
    }

    // insert node by going through tree ordering and finding node's parent
    public void insert(int value) {
        if (root == null) {
            root = new Node(value);
            return;
        }

        Node parent = findInsertParent(value, root, null);
        if (parent == null) {
            return;
        }

        if (parent.value > value) {
            parent.left = new Node(value);
        }
        else if (parent.value < value) {
            parent.right = new Node(value);
        }
    }

    // returns parent of potential node (by its value), where new node could be placed
    // or returns null when node with given value already exists
    private Node findInsertParent(int value, Node currentNode, Node parentNode) {
        if (currentNode == null) {
            return parentNode;
        }

        if (currentNode.value == value) {
            return null;
        }
        else if (currentNode.value > value) {
            return findInsertParent(value, currentNode.left, currentNode);
        }
        else {
            return findInsertParent(value, currentNode.right, currentNode);
        }
    }

    // binary tree node representation holding int value
    private static class Node {
        Node left, right;
        int value;

        public Node(int value) {
            this.value = value;
            left = right = null;
        }

        @Override
        public String toString() {
            return Integer.toString(value);
        }
    }

    public Iterator iterator() {
        return new Iterator() {
            // help stack structure for holding needed information for tree traversal
            private final Stack<Node> stack = new Stack<Node>();
            { // static init to the lowest value
                enqueueNodeAndItsLeftBranch(root);
            }

            public boolean hasNext() {
                return !stack.isEmpty();
            }

            public Node next() throws NoSuchElementException {
                if (hasNext()) {
                    Node currentNode = stack.pop();
                    enqueueNodeAndItsLeftBranch(currentNode.right);
                    return currentNode;
                }
                throw new NoSuchElementException();
            }

            // insert given node and all its left branch into stack data structure
            private void enqueueNodeAndItsLeftBranch(Node node) {
                while (node != null) {
                    stack.push(node);
                    node = node.left;
                }
            }
        };
    }
}