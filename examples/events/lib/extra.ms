tick = function (args)
    if args.pass == true then
        print "tick"
    end if
end function

click = function (args)
    print "moving to " + args.pos
    move(args.pos.x, args.pos.y,  args.pos.z)
end function

echo = function (args)
    print args.msg
end function