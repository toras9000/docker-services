<?php

use BookStack\Facades\Theme;
use BookStack\Theming\ThemeEvents;
use BookStack\Entities\Controllers;
use BookStack\Entities\Models;
use Illuminate\Routing\Router;
use League\CommonMark\Environment\Environment;
use League\CommonMark\Extension\Footnote\FootnoteExtension;
use League\CommonMark\Extension\Autolink\AutolinkExtension;
use League\CommonMark\Extension\DisallowedRawHtml\DisallowedRawHtmlExtension;

// Register a permalink endpoint
Theme::listen(ThemeEvents::ROUTES_REGISTER_WEB, function (Router $router)
{
    $router->get('/link/shelves/{id}', function (string $id)
    {
        $entity = Models\Bookshelf::query()->where('id', '=', $id)->first();
        return isset($entity) ? redirect($entity->getUrl()) : abort(404);
    })->whereNumber('id');

    $router->get('/link/books/{id}', function (string $id)
    {
        $entity = Models\Book::query()->where('id', '=', $id)->first();
        return isset($entity) ? redirect($entity->getUrl()) : abort(404);
    })->whereNumber('id');

    $router->get('/link/chapters/{id}', function (string $id)
    {
        $entity = Models\Chapter::query()->where('id', '=', $id)->first();
        return isset($entity) ? redirect($entity->getUrl()) : abort(404);
    })->whereNumber('id');

    $router->get('/link/pages/{id}', function (string $id)
    {
        $entity = Models\Page::query()->where('id', '=', $id)->first();
        return isset($entity) ? redirect($entity->getUrl()) : abort(404);
    })->whereNumber('id');
});


Theme::listen(ThemeEvents::COMMONMARK_ENVIRONMENT_CONFIGURE, function(Environment $environment)
{
    $environment->addExtension(new FootnoteExtension());
    $environment->addExtension(new AutolinkExtension());
    $environment->addExtension(new DisallowedRawHtmlExtension());
    $environment->mergeConfig([
        'renderer' => [
            'soft_break'      => "\n",
        ],
    ]);
});
