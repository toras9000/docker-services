<div class="entity-meta">
    @if($entity->isA('revision'))
        <div class="entity-meta-item">
            @icon('history')
            <div>
                {{ trans('entities.pages_revision') }}
                {{ trans('entities.pages_revisions_number') }}{{ $entity->revision_number == 0 ? '' : $entity->revision_number }}
            </div>
        </div>
    @endif

    @if ($entity->isA('page'))
        <a href="{{ $entity->getUrl('/revisions') }}" class="entity-meta-item">
            @icon('history'){{ trans('entities.meta_revision', ['revisionCount' => $entity->revision_count]) }}
        </a>
    @endif

    @if ($entity->ownedBy && $entity->owned_by !== $entity->created_by)
        <div class="entity-meta-item">
            @icon('user')
            <div>
                {!! trans('entities.meta_owned_name', [
                    'user' => "<a href='{$entity->ownedBy->getProfileUrl()}'>".e($entity->ownedBy->name). "</a>"
                ]) !!}
            </div>
        </div>
    @endif

    @if ($entity->createdBy)
        <div class="entity-meta-item">
            @icon('star')
            <div>
                {!! trans('entities.meta_created_name', [
                    'timeLength' => '<span title="'.$entity->created_at->toDayDateTimeString().'">'.$entity->created_at->diffForHumans() . '</span>',
                    'user' => "<a href='{$entity->createdBy->getProfileUrl()}'>".e($entity->createdBy->name). "</a>"
                ]) !!}
            </div>
        </div>
    @else
        <div class="entity-meta-item">
            @icon('star')
            <span title="{{$entity->created_at->toDayDateTimeString()}}">{{ trans('entities.meta_created', ['timeLength' => $entity->created_at->diffForHumans()]) }}</span>
        </div>
    @endif

    @if ($entity->updatedBy)
        <div class="entity-meta-item">
            @icon('edit')
            <div>
                {!! trans('entities.meta_updated_name', [
                    'timeLength' => '<span title="' . $entity->updated_at->toDayDateTimeString() .'">' . $entity->updated_at->diffForHumans() .'</span>',
                    'user' => "<a href='{$entity->updatedBy->getProfileUrl()}'>".e($entity->updatedBy->name). "</a>"
                ]) !!}
            </div>
        </div>
    @elseif (!$entity->isA('revision'))
        <div class="entity-meta-item">
            @icon('edit')
            <span title="{{ $entity->updated_at->toDayDateTimeString() }}">{{ trans('entities.meta_updated', ['timeLength' => $entity->updated_at->diffForHumans()]) }}</span>
        </div>
    @endif

    @if($referenceCount ?? 0)
        <a href="{{ $entity->getUrl('/references') }}" class="entity-meta-item">
            @icon('reference')
            <div>
                {{ trans_choice('entities.meta_reference_count', $referenceCount, ['count' => $referenceCount]) }}
            </div>
        </a>
    @endif

    @if($watchOptions?->canWatch())
        @if($watchOptions->isWatching())
            @include('entities.watch-controls', [
                'entity' => $entity,
                'watchLevel' => $watchOptions->getWatchLevel(),
                'label' => trans('entities.watch_detail_' . $watchOptions->getWatchLevel()),
                'ignoring' => $watchOptions->getWatchLevel() === 'ignore',
            ])
        @elseif($watchedParent = $watchOptions->getWatchedParent())
            @include('entities.watch-controls', [
                'entity' => $entity,
                'watchLevel' => $watchOptions->getWatchLevel(),
                'label' => trans('entities.watch_detail_parent_' . $watchedParent->type . ($watchedParent->ignoring() ? '_ignore' : '')),
                'ignoring' => $watchedParent->ignoring(),
            ])
        @endif
    @endif

    @if (!$entity->draft)
        @php
                 if ($entity->isA('bookshelf')) { $category = 'shelves';  }
            else if ($entity->isA('book'))      { $category = 'books';    }
            else if ($entity->isA('chapter'))   { $category = 'chapters'; }
            else if ($entity->isA('page'))      { $category = 'pages';    }
        @endphp
        @if ($category)
            <a href="/link/{{ $category }}/{{ $entity->id }}" class="entity-meta-item">
                @icon('link')
                <div>
                    Permalink
                </div>
            </a>
        @endif
    @endif
</div>