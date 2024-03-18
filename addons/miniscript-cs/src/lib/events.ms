_events = []

pollEvents = function ()
    while true
        while _events.len > 0
            _nextEvent = _events.pull()
            _nextEvent.invoke(_nextEvent.args)
        end while
        yield
    end while
end function
