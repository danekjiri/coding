package cz.cuni.mff.danekji1.programs;

import cz.cuni.mff.danekji1.utils.BinarySearchTree;

public class Main {
    public static void main(String[] args) {

        // define const error message and tree data structure
        final String ERROR_MESSAGE = "INPUT ERROR";
        var bst = new BinarySearchTree();

        // process arguments and insert in the BST
        for (String s : args) {
            try {
                bst.insert(Integer.parseInt(s));
            }
            catch (NumberFormatException ex) {
                System.out.println(ERROR_MESSAGE);
                return;
            }
        }

        // go through the whole tree and print all elements in inorder (ordered) sequence using iterator
        for (Object o : bst) {
            System.out.println(o.toString());
        }
    }
}