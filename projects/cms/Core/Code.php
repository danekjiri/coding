<?php

namespace Core;

enum Code: int {
    case OK = 200;
    case SEE_OTHER = 303;   // redirects
    case BAD_REQUEST = 400;
    case NOT_FOUND = 404;
    case INTERNAL_ERR = 500;
}