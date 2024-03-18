tick = function (args)
    if args.pass == true then
        print "tick"
    end if
end function

click = function (args)
    print "clicked"
end function

echo = function (args)
    print args.msg
end function